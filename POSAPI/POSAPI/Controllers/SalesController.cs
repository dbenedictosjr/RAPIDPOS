using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using POS.Domain;
using POS.Infrastructure.Interfaces;

namespace POSAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IRepository<PS_DOC_HDR> _pS_DOC_HDRRepository;

        public SalesController(IRepository<PS_DOC_HDR> pS_DOC_HDRRepository)
        {
            _pS_DOC_HDRRepository = pS_DOC_HDRRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PS_DOC_HDR input)
        {
            try
            {
                var createResponse = await _pS_DOC_HDRRepository.Create(input);

                var responseBody = JsonConvert.SerializeObject(createResponse);

                var response = new ContentResult
                {
                    Content = responseBody,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS,POST");
                Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                return response;
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine(ex.Message);

                var errorResponse = new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new { error = "An error occurred while creating the sales." }),
                    ContentType = "application/json",
                    StatusCode = 500
                };

                return errorResponse;
            }
        }
    }
}
