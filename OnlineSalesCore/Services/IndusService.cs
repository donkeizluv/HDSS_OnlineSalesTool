using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;
using OnlineSalesCore.Options;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace OnlineSalesCore.Services
{
    public class IndusService : IIndusService
    {
        private static IDictionary<string, string> _scriptDict = new Dictionary<string, string>();
        private readonly IndusOptions _options;
        private readonly IHostingEnvironment _env;
        public IndusService(IOptions<IndusOptions> options,
            IHostingEnvironment env)
        {
            _env = env;
            _options = options.Value;
        }
        public async Task<IndusContractDTO> GetContract(string contractNumber)
        {
            if (string.IsNullOrEmpty(contractNumber))
            {
                throw new ArgumentException("message", nameof(contractNumber));
            }
            //Inject connection if usage gets more complicated
            using (var conn = new OracleConnection(_options.ConnectionString))
            {
                await conn.OpenAsync();
                var cmd = GetContractDetailCommand(contractNumber);
                var contracts = await conn.QueryAsync<IndusContractDTO>(cmd);
                if (!contracts.Any()) return null;
                var contract = contracts.First();
                return contract;
            }
        }
        private CommandDefinition GetContractDetailCommand(string contractNumber)
        {
            return new CommandDefinition( 
                ReadScript(_options.ContractDetailScript, "contract_detail")
                .Replace("{contract}", contractNumber));
        }
        private string ReadScript(string path, string scriptName)
        {
            if(!_scriptDict.ContainsKey(scriptName))
            {
                //Cache script
                var script = File.ReadAllText(Path.Combine(_env.ContentRootPath, path), Encoding.UTF8);
                _scriptDict.Add(scriptName, script);
                return script;
            }
            return _scriptDict[scriptName];
        }
    }
}
