// -
// <copyright file="ServiceResult.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.Net;

namespace Hawaii.Services.Client
{
    /// <summary>
    /// A base class for all Hawaii service result classes. 
    /// Various Hawaii service result classes will represent the result corresponding to different type of Hawaii service calls. 
    /// This class contains functionality common to all Hawaii service result classes.
    /// </summary>
    public abstract class ServiceResult 
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        public ServiceResult() :
            this(null, Status.Unspecified, null)
        {
        }

         /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="stateObject">Specifies a user-defined object.</param>
        public ServiceResult(object stateObject) :
            this(stateObject, Status.Unspecified, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="stateObject">Specifies a user-defined object.</param>
        /// <param name="status">Specifies the status of the service call.</param>
        public ServiceResult(object stateObject, Status status) :
            this(stateObject, status, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="stateObject">Specifies a user-defined object.</param>
        /// <param name="status">Specifies the status of the service call.</param>
        /// <param name="exception">An exception instance used if an error occured during the service call.</param>
        public ServiceResult(object stateObject, Status status, Exception exception)
        {
            this.Status = status;
            this.Exception = exception;
            this.StateObject = stateObject;
        }

        /// <summary>
        /// Gets or sets the error exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the status of the service call.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets a user-defined object.
        /// </summary>
        public object StateObject { get; set; }
    }
}
