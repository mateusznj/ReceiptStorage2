// -
// <copyright file="ServiceAgent.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hawaii.Services.Client
{
    /// <summary>
    /// A base class for all Hawaii service agent classes. 
    /// These agents are wrapping the communication tasks specific to each service.
    /// ServiceAgent provides functionality common to all these clases.
    /// </summary>
    public abstract class ServiceAgent<T> where T : ServiceResult, new()
    {
        /// <summary>
        /// Initializes a new instance of the ServiceAgent class.
        /// </summary>
        public ServiceAgent() :
            this(HttpMethod.Get, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceAgent class.
        /// </summary>
        /// <param name="requestMethod">Specifies a http request method.</param>
        /// <param name="stateObject">Specifies a user-defined object.</param> 
        public ServiceAgent(HttpMethod requestMethod, object stateObject)
        {
            this.RequestMethod = requestMethod;
            this.Uri = null;
            this.ClientIdentity = null;
            this.Result = new T();
            this.Result.StateObject = stateObject;
        }

        /// <summary>
        /// OnCompleteDelegate delegate type definition.
        /// </summary>
        /// <param name="result">
        /// Returns nothing.
        /// </param>
        public delegate void OnCompleteDelegate(T result);

        /// <summary>
        /// Gets or sets OnComplete handler.
        /// </summary>
        protected OnCompleteDelegate OnComplete { get; set; }

        /// <summary>
        /// Gets or sets service result.
        /// </summary>
        protected T Result { get; set; }

        /// <summary>
        /// Gets or sets service Uri.
        /// </summary>
        protected Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method (GET, POST, PUT or DELETE).
        /// </summary>
        protected HttpMethod RequestMethod { get; set; }

        /// <summary>
        /// Gets or sets the client identity.
        /// </summary>
        protected ClientIdentity ClientIdentity { get; set; }

        /// <summary>
        /// This method initiates the asynchronous service call.
        /// </summary>
        /// <param name="handlerMethod">
        /// The on complete" callback that will be invoked after the service call completes.
        /// </param>
        public void ProcessRequest(OnCompleteDelegate handlerMethod)
        {
            try
            {
                if (handlerMethod != null)
                {
                    this.OnComplete = handlerMethod;
                }

                // Create the Http request.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.Uri);

                // Set http method.
                request.Method = this.RequestMethod.ToString().ToUpper();

                string identityToken = null;
                if (this.ClientIdentity != null)
                {
                    // In case of valid identity provided by the client,
                    // get the identity token
                    identityToken = this.ClientIdentity.GetEncodedToken();
                }

                if (!string.IsNullOrEmpty(identityToken))
                {
                    // if the identity token is not null, set the authorization header.
                    request.Headers[HttpRequestHeader.Authorization] = identityToken;
                }

                if (this.RequestMethod == HttpMethod.Post)
                {
                    request.BeginGetRequestStream(new AsyncCallback(this.RequestCallback), request);
                }
                else
                {
                    request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), request);
                }
            }
            catch (Exception ex)
            {
                this.Result.Status = Status.Error;
                this.Result.Exception = ex;

                // In case any error exception at UI thread context, OnCompleteRequest should be 
                // called by UI thread.
                this.OnCompleteRequest();
            }
        }

        /// <summary>
        /// Deserializes the response stream.
        /// </summary>
        /// <typeparam name="TResult">Result object.</typeparam>
        /// <param name="responseStream">Server response stream.</param>
        /// <returns>Deserailized object.</returns>
        protected static TResult DeserializeResponse<TResult>(Stream responseStream) where TResult : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TResult));
                return serializer.Deserialize(responseStream) as TResult;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Invalid response received from server.", ex);
            }
        }

        /// <summary>
        /// This method must be implemented by all classes that inherit from ServiceAgent. 
        /// It will provide the POST data that has to be sent to the service if the Http Method used is POST.
        /// </summary>
        /// <returns>Returns the POST data in byte array format.</returns>
        protected virtual byte[] GetPostData()
        {
            return null;
        }

        /// <summary>
        /// This method is called after the response sent by the server is received by the client.
        /// It allows classes that inherit from ServiceAgent to do their own processing of 
        /// the data received from the server.
        /// </summary>
        /// <param name="responseStream">
        /// The response stream containing response data that is received from the server.
        /// </param>
        protected virtual void ParseOutput(Stream responseStream)
        {
        }

        /// <summary>
        /// A virtual method. It is invoked after completing the service request.
        /// The implementation of this base class will invoke the client's "on complete" callback method.
        /// Classes that inherit from ServiceAgent can oveerite this method to further process the service 
        /// call result before calling the client's "on complete" callback method.
        /// </summary>
        protected virtual void OnCompleteRequest()
        {
            if (this.OnComplete != null)
            {
                // Call the UI's on completion method.
                this.OnComplete(this.Result);
            }
        }

        /// <summary>
        /// Callback method called after request.BeginGetRequestStream completes.
        /// </summary>
        /// <param name="asyncResult">
        /// An asyncResult object.
        /// </param>
        private void RequestCallback(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null, "IAsyncResult object is null");
            Debug.Assert(asyncResult.AsyncState != null, "IAsyncResult.AsyncState object is null");

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Debug.Assert(request != null, "HttpWebRequest object is null");

            try
            {
                using (Stream stream = request.EndGetRequestStream(asyncResult))
                {
                    if (stream == null)
                    {
                        throw new Exception("Null/Invalid request stream received from server.");
                    }

                    // Get the input from the service client.
                    byte[] inputBuffer = this.GetPostData();

                    // Step 3: POST data, for a POST request.
                    if (inputBuffer != null && inputBuffer.Length != 0)
                    {
                        stream.Write(inputBuffer, 0, inputBuffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Result.Status = Status.Error;
                this.Result.Exception = ex;

                // Call the OnCompleteRequest handler in case any error handling request.
                this.OnCompleteRequest();
            }

            request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), request);
        }

        /// <summary>
        /// Callback method called after request.BeginGetResponse completes.
        /// </summary>
        /// <param name="asyncResult">
        /// An asyncResult object.
        /// </param>
        private void ResponseCallback(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null, "IAsyncResult object is null");
            Debug.Assert(asyncResult.AsyncState != null, "IAsyncResult.AsyncState object is null");

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Debug.Assert(request != null, "HttpWebRequest object is null");

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult))
                {
                    if (response == null)
                    {
                        // Create and set the error exception.
                        throw new Exception("Invalid response from server.");
                    }

                    this.Result.Status = this.ConvertStatus(response.StatusCode);
                    if (this.Result.Status != Status.Success)
                    {
                        // Create and set the error exception.
                        throw new Exception("Invalid result status from server.");
                    }

                    // Calls client method to interprete the result buffer.
                    this.ParseOutput(response.GetResponseStream());

                    // The following code will be never hit, but kept it for safety.
                    if (this.Result.Exception != null &&
                        this.Result.Status == Status.Success)
                    {
                        // This is be an invalid combination.
                        Debug.Assert(false, "Result status can't be success when exception indicates an error.");
                        throw new Exception("Invalid response stream received from server.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Result.Status = Status.Error;
                this.Result.Exception = ex;
            }
            finally
            {
                // Calls client method to call service event hanlder.
                this.OnCompleteRequest();
            }
        }

        /// <summary>
        /// Method converts HttpStatusCode to Status.
        /// </summary>
        /// <param name="statusCode">Service http status code.</param>
        /// <returns>Hawaii Status code.</returns>
        private Status ConvertStatus(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    return Status.Success;

                case HttpStatusCode.InternalServerError:
                    return Status.InternalServerError;

                default:
                    return Status.Error;
            }
        }
    }
}
