using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Phone;

namespace ReceiptStorage.Extensions
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[])
            {
                //MemoryStream memoryStream = new MemoryStream(value as byte[]);
                //WriteableBitmap writeBitmap = PictureDecoder.DecodeJpeg(memoryStream);
                //return writeBitmap;
                MemoryStream memStream = new MemoryStream((byte[]) value);
                memStream.Seek(0, SeekOrigin.Begin);
                BitmapImage empImage = new BitmapImage();
                empImage.SetSource(memStream);
                return empImage;
            }
            else{
                return null;
            }
    }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static byte[] ConvertToBytes(String imageLocation)
        {
            byte[] imgByteArray = new byte[] {};
            StreamResourceInfo sri = Application.GetResourceStream(new Uri(imageLocation, UriKind.Relative));
            if (sri !=null ){  
                BinaryReader binary = new BinaryReader(sri.Stream);

                imgByteArray = binary.ReadBytes((int)(sri.Stream.Length));

                binary.Close();
                binary.Dispose();
            }
            return imgByteArray;
        }

        public static WriteableBitmap ConvertToImage(Byte[] inputBytes)
        {
            MemoryStream ms = new MemoryStream(inputBytes);
            WriteableBitmap img = new WriteableBitmap(400, 400);

            img.LoadJpeg(ms);

            return (img);
        }

    }


    
}
