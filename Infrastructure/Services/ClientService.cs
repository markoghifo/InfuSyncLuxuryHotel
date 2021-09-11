using AutoMapper;
using Core.DI;
using Core.Models;
using Infrastructure.Documents;
using Infrastructure.Helpers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMapper _mapper;

        public ClientService(IHotelDatabaseSettings settings, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _clients = database.GetCollection<Client>("Clients");
        }

        public IEnumerable<ClientDTO> Get()
        {
            var clients = _clients.Find(client => true).ToList();
            var res = Helpers.EntityMapper.Mapper.Map<List<ClientDTO>>(clients);
            return res;
        }

        public Client Get(string id) =>
            _clients.Find<Client>(client => client.Id == id).FirstOrDefault();

        public async Task<ClientDTO> CreateAsync(ClientDTO data)
        {
            var client = _mapper.Map<Client>(data);
            await _clients.InsertOneAsync(client);
            var res = EntityMapper.Mapper.Map<ClientDTO>(client);
            return res;
        }

        public async Task<ClientDTO> UpdateClientStateAsync(string id, ClientStates state)
        {
            var clientDto = Get(id);
            if (clientDto != null)
            {
                var client = EntityMapper.Mapper.Map<Client>(clientDto);
                var filter = Builders<Client>.Filter.Eq("_id", id);
                var update = Builders<Client>.Update
                    .Set(nameof(ClientDTO.States), state);
                await _clients.UpdateOneAsync(filter, update);

                return EntityMapper.Mapper.Map<ClientDTO>(client);
            }
            throw new Exception("Client not found");
        }
    }
}
