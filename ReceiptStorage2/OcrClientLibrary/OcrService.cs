// -
// <copyright file="OcrService.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Hawaii.Services.Client.Ocr
{
    /// <summary>
    /// Helper class that provides access to the OCR service.
    /// </summary>
    public static class OcrService
    {
        /// <summary>
        /// Specifies the service host name. This will be used to create the service url.
        /// </summary>
        public const string HostName = @"http://ocr2.hawaii-services.net";

        /// <summary>
        /// Specifies a signature for the REST method that executes the OCR processing.
        /// </summary>
        public const string ServiceSignature = "OCR";

        /// <summary>
        /// Helper method to initiate the call to the Hawaii OCR service.
        /// </summary>
        /// <param name="hawaiiAppId">Specifies the Hawaii Application Id.</param>
        /// <param name="imageBuffer">
        /// Specifies a buffer containing an image that has to be processed.
        /// The image must be in JPEG format.
        /// </param>
        /// <param name="onComplete">Specifies an on complete callback method.</param>
        public static void RecognizeImageAsync(
            string hawaiiAppId, 
            byte[] imageBuffer,
            ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete)
        {
            OcrService.RecognizeImageAsync(
                hawaiiAppId, 
                imageBuffer, 
                onComplete, 
                null);
        }

        /// <summary>
        /// Helper method to recognize an image.
        /// </summary>
        /// <param name="hawaiiAppId">Specifies the Hawaii Application Id.</param>
        /// <param name="imageBuffer">
        /// Specifies a buffer containing an image that has to be processed.
        /// The image must be in JPEG format.
        /// </param>
        /// <param name="onComplete">Specifies an on complete callback method.</param>
        /// <param name="stateObject">Specifies a user defined object which will be provided in the call of the callback method.</param>
        public static void RecognizeImageAsync(
            string hawaiiAppId, 
            byte[] imageBuffer,
            ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete, 
            object stateObject)
        {
            OcrAgent agent = new OcrAgent(
                OcrService.HostName,
                hawaiiAppId, 
                imageBuffer, 
                stateObject);

            agent.ProcessRequest(onComplete);
        }
    }
}

