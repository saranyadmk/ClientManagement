using ClientManagement.Database;
using ClientManagement.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace ClientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients(string term, string sort, int page = 1, int limit = 2)
        {
            var clients = await _clientRepository.GetAllClientsAsync();
            if (clients != null && clients.Count > 0)
            {
                var clientResult = await _clientRepository.GetClients(term, sort, page, limit);

                Response.Headers.Add("X-Total-Count", clientResult.TotalCount.ToString()); //6 records
                Response.Headers.Add("X-Total-Pages", clientResult.TotalPages.ToString()); //3 pages

                return Ok(clientResult.Clients);
            }
            else
            {
                return NotFound("Clients not found");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Enter a valid id");
            }

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound("Client not found");
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Client not found");
            }

            var id = await _clientRepository.AddNewClientAsync(client);
            return CreatedAtAction(nameof(GetClientById), new { id = id, Controller = "Client" }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClientDetails([FromRoute] int id, [FromBody] Client client)
        {
            if (id <= 0)
            {
                return BadRequest("Enter valid id");
            }

            var Clientt = await _clientRepository.GetClientByIdAsync(id);

            if (Clientt == null)
            {
                return NotFound("Client not fonund");
            }

            await _clientRepository.UpdateClientAsync(id, client);
            return Ok("Details are updated");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateClientPatch([FromBody] JsonPatchDocument patchDocument, [FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Enter valid id");
            }

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound("Client not found");
            }

            await _clientRepository.UpdateClientUSingPatchAsync(id, patchDocument);
            return Ok("Details are updated through patch");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Enter valid id");
            }

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound("Client not found");
            }

            await _clientRepository.DeleteClientAsync(id);
            return Ok("Client details are deleted");
        }
    }
}
