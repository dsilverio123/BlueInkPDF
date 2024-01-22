using PdfiumViewer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfDocumentSharp = PdfSharp.Pdf.PdfDocument; // Alias for PdfSharp.Pdf.PdfDocument
using System.Drawing.Imaging;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private string selectedPdfPath = string.Empty; // Store the selected PDF file path

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";
            openFileDialog.Title = "Select a PDF File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedPdfPath = openFileDialog.FileName;
                lblOriginalDirectory.Text = "Original PDF Directory: " + Path.GetDirectoryName(selectedPdfPath);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedPdfPath))
            {
                // Get the directory of the selected PDF file
                string directory = Path.GetDirectoryName(selectedPdfPath);

                // Generate a new file name in the same directory with "_converted" appended
                string convertedFileName = Path.GetFileNameWithoutExtension(selectedPdfPath) + "_converted.pdf";
                string savePdfPath = Path.Combine(directory, convertedFileName);

                // Load the PDF document using PdfiumViewer
                using (PdfiumViewer.PdfDocument pdfDocument = PdfiumViewer.PdfDocument.Load(selectedPdfPath))
                {
                    if (pdfDocument.PageCount > 0)
                    {
                        // Create a new PDF document for saving the converted page using PdfSharp
                        PdfDocumentSharp convertedPdfDocument = new PdfDocumentSharp();

                        for (int pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
                        {
                            PdfPage convertedPage = convertedPdfDocument.AddPage();
                            using (XGraphics gfx = XGraphics.FromPdfPage(convertedPage))
                            {
                                // Define the output size (you can adjust these values)
                                int outputWidth = 12000; // Desired width
                                int outputHeight = 16000; // Desired height

                                // Calculate the scaling factor to fit the PDF content within the output size
                                float scaleX = (float)outputWidth / pdfDocument.PageSizes[pageIndex].Width;
                                float scaleY = (float)outputHeight / pdfDocument.PageSizes[pageIndex].Height;
                                float scale = Math.Min(scaleX, scaleY);

                                // Calculate the new width and height based on the scaling factor
                                int newWidth = (int)(pdfDocument.PageSizes[pageIndex].Width * scale);
                                int newHeight = (int)(pdfDocument.PageSizes[pageIndex].Height * scale);

                                // Render the PDF page with the calculated dimensions
                                Bitmap pageImage = (Bitmap)pdfDocument.Render(pageIndex, newWidth, newHeight, true);

                                if (pageImage != null)
                                {
                                    try
                                    {
                                        ModifyImage(pageImage); // Apply your logic to modify the image

                                        using (MemoryStream stream = new MemoryStream())
                                        {
                                            // Save as TIFF format with compression
                                            SaveTiffWithCompression(pageImage, stream, EncoderValue.CompressionLZW); // You can adjust the compression level here

                                            // Convert the Bitmap to a stream
                                            XImage modifiedImage = XImage.FromStream(stream); // Create an XImage from the stream

                                            // Draw the modified image on the converted page
                                            gfx.DrawImage(modifiedImage, 0, 0, convertedPage.Width, convertedPage.Height);
                                        }
                                    }
                                    finally
                                    {
                                        pageImage.Dispose(); // Dispose of the Bitmap to release resources
                                    }
                                }
                            }
                        }

                        // Save the converted PDF document
                        convertedPdfDocument.Save(savePdfPath);

                        MessageBox.Show("PDF Converted with Modifications and Saved as PDF!", "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The PDF has no pages.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a PDF file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SaveTiffWithCompression(Bitmap bmp, Stream stream, EncoderValue compressionLevel)
        {
            ImageCodecInfo tiffCodecInfo = GetTiffEncoderInfo();

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)compressionLevel); // Adjust the compression level here

            bmp.Save(stream, tiffCodecInfo, encoderParams);
        }

        private ImageCodecInfo GetTiffEncoderInfo()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Tiff.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void ModifyImage(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    if (!IsWhite(pixelColor))
                    {
                        bmp.SetPixel(x, y, Color.Blue);
                    }
                }
            }
        }

        private bool IsWhite(Color color)
        {
            return color.R > 240 && color.G > 240 && color.B > 240;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblOriginalDirectory_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void ModifyImage32bppArgb(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    if (!IsWhite(pixelColor))
                    {
                        bmp.SetPixel(x, y, Color.Blue);
                    } 
                }
            }
        }
    }
}
