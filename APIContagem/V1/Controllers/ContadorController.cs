﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using APIContagem.V1.Models;

namespace APIContagem.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ContadorController : ControllerBase
    {
        private static readonly Contador _CONTADOR = new Contador();
        private readonly ILogger<ContadorController> _logger;
        private readonly IConfiguration _configuration;

        public ContadorController(ILogger<ContadorController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet, MapToApiVersion("1.1")]
        public ResultadoContador GetV1_1()
        {
            int valorAtualContador;
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();
                valorAtualContador = _CONTADOR.ValorAtual;
            }

            return GetResultadoContador(valorAtualContador);
        }

        [HttpGet, MapToApiVersion("1.0")]
        public ResultadoContador GetV1_0()
        {
            int valorAtualContador;
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();
                valorAtualContador = _CONTADOR.ValorAtual;
            }

            if (valorAtualContador % 4 == 0)
            {
                _logger.LogError("Simulando falha...");
                throw new Exception("Simulação de falha!");
            }

            return GetResultadoContador(valorAtualContador);
        }

        private ResultadoContador GetResultadoContador(int valorAtualContador)
        {
            _logger.LogInformation($"Contador - Valor atual: {valorAtualContador}");

            return new()
            {
                ValorAtual = valorAtualContador,
                Local = _CONTADOR.Local,
                Kernel = _CONTADOR.Kernel,
                TargetFramework = _CONTADOR.TargetFramework,
                Mensagem = _configuration["MensagemVariavel"]
            };
        }
    }
}