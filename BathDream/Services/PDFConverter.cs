using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Services
{
    public class PDFConverter
    {
        readonly IConverter converter;
        public PDFConverter()
        {
            converter = new SynchronizedConverter(new PdfTools()); 
        }

        /// <summary>
        /// Преобразует страницу в PDF файл
        /// </summary>
        /// <param name="link">Ссылка на страницу</param>
        public byte[] Convert(string link, string name)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = name,
            };
            var objectSettings = new ObjectSettings
            {
                Page = link,
                WebSettings = { DefaultEncoding = "", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "main.css") },
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = converter.Convert(pdf);
            return file;
        } 
    }
}
