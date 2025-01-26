using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_function_validacao_cpf
{
    public static class fnValidaCpf
    {
        [FunctionName("fnValidaCpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("iniciando a validação do cpf.");

            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            if(data == null) {
                    return new BadRequestObjectResult("informar o cpf");
            }
            string cpf = data?.cpf;

            if (ValidaCPF(cpf) == false)
            {
                return new BadRequestObjectResult("CPF inválido");
            }

            var responseMessage = "CPF válido.";

            return new OkObjectResult(responseMessage);
        }

        public static bool ValidaCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;
    
        
            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);
        
            int primeiroDigito = 11 - (soma % 11);
            if (primeiroDigito >= 10)
                primeiroDigito = 0;
        
            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);
        
            int segundoDigito = 11 - (soma % 11);
            if (segundoDigito >= 10)
                segundoDigito = 0;
        
            return cpf.EndsWith($"{primeiroDigito}{segundoDigito}");
        }
        
    }

    
}
