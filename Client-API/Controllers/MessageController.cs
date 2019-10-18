
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_API.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client_API.Controllers
{

    public class DTOMessagePost
    {
        public string Message { get; set; }
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<Object> Get()
        {
            return RabbitMQClient.Instance.GetCorrelateIDAndQueueName();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public Object SendMessageAsync(dynamic dynParam)
        {

            DTOMessagePost reqdata = JsonConvert.DeserializeObject<DTOMessagePost>(dynParam.reqdata.ToString());
            // Console.WriteLine("DAy adasdasdsadasdasdasdasdsad");
            Console.WriteLine(reqdata.Message);
            // return RabbitMQClient.Instance.SendMessage("go");
            return RabbitMQClient.Instance.SendMessageWithAsync(reqdata.Message);

        }

        [HttpPost]
        public Object SendMessageSync(dynamic dynParam)
        {

            DTOMessagePost reqdata = JsonConvert.DeserializeObject<DTOMessagePost>(dynParam.reqdata.ToString());
            // Console.WriteLine("DAy adasdasdsadasdasdasdasdsad");
            Console.WriteLine(reqdata.Message);
            // return RabbitMQClient.Instance.SendMessage("go");
            return new {message = RabbitMQClient.Instance.SendMessageWithSync(reqdata.Message)};

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
