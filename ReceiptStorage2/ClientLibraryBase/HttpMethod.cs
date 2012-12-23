// -
// <copyright file="HttpMethod.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;

namespace Hawaii.Services.Client
{
    /// <summary>
    /// Specifies the HTTP method that is used when communicating with the server.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Represents the Http Get method.
        /// </summary>
        Get,
 
        /// <summary>
        /// Represents the Http Post method.
        /// </summary>
        Post,

        /// <summary>
        /// Represents the Http Put method.
        /// </summary>
        Put, 

        /// <summary>
        /// Represents the Http Delete method.
        /// </summary>
        Delete 
    }
}
