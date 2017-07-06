using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Email.DataAccess.IRepositories;
using PriceParse;

namespace Email.Service.Web.Controllers
{
    public class ApiFileMediaController : ApiController
    {
        private readonly IFileRepository _repository;

        public ApiFileMediaController(IFileRepository repository)
        {
            _repository = repository;
        }

        public HttpResponseMessage GetFile(string path= @"C:\DownloadedFiles\dmitrij82@ezex.ru\29 марта 2017 г.\Прайс-лист РУСЬ (Каховка).xlsx")
        {
            var file = _repository.GetFileByPath(path);
            if (file.Length<=0)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(file)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = path.Split('\\').Last(),
                Size = file.Length
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;
        }

    }
}
