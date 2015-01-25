using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using xiiiCommonDals;
using xiiiCommonModels;
using xiiiWebApi.Models;

namespace xiiiWebApi.Controllers
{
    public class SmsController : ApiController
    {
        private ISmsRepository _repository;

        public SmsController(ISmsRepository repository)
        {
            _repository = repository;
        }

        public IHttpActionResult Get(Guid guid)
        {
            var result = _repository.Read(guid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        public IHttpActionResult Get(string to, Guid guid)
        {
            var result = _repository.Read(to, guid);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/Sms
        public IHttpActionResult Post(SendSmsBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("no parameters");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = DateTime.UtcNow;
            var result = new SmsMessage { Created = created, From = model.From, Message = model.Message, To = model.To };
            _repository.InsertThenQueue(result);
            return Created(result.Guid.ToString(), result);
        }
    }
}
