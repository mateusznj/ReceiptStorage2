// -
// <copyright file="Status.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Hawaii.Services.Client
{
    /// <summary>
    /// Describes the status of a Hawaii service call.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Defaut status code.
        /// </summary>
        Unspecified, 

        /// <summary>
        /// Specifies that the service call completed successfully.
        /// </summary>
        Success,
        
        /// <summary>
        /// Specifies that the service call completed with an internal server error.
        /// </summary>
        InternalServerError,

        /// <summary>
        /// Specifies that the service call completed with an error.
        /// </summary>
        Error
    }
}
