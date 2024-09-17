using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionAPI.Controllers;
using GestionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionAPI.Test
{
    public class ParticipantesTests
    {
        //agregar participante cuando todo es válido
        [Fact]
        public async Task PostParticipante_AgregaParticipante_ParticipanteValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = "Abigail Acuña",
                CorreoParticipante = "abigail.acuna@gmail.com",
                EventoId = 1
            };

            var result = await controller.PostParticipante(nuevoParticipante);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var participante = Assert.IsType<Participante>(createdResult.Value);
            Assert.Equal("Abigail Acuña", participante.NombreParticipante);
        }
        //ver participante cuando id es válido
        [Fact]
        public async Task GetParticipante_RetornaParticipante_CuandoIdValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = "Abigail Acuña",
                CorreoParticipante = "abigail.acuna@gmail.com",
                EventoId = 1
            };
            context.Participantes.Add(nuevoParticipante);
            await context.SaveChangesAsync();
            var result = await controller.GetParticipante(nuevoParticipante.Id);
            var actionResult = Assert.IsType<ActionResult<Participante>>(result);
            var returnValue = Assert.IsType<Participante>(actionResult.Value);
            Assert.Equal("Abigail Acuña", returnValue.NombreParticipante);
        }
        [Fact]
        //ver participante cuando id no es válido
        public async Task GetParticipante_RetornaParticipante_CuandoIdNoValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
           
           
            var result = await controller.GetParticipante(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }
        //Agregar participante cuando nombre es nulo
        [Fact]
        public async Task PostParticipante_NoAgregaParticipante_NombreNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = null,
                CorreoParticipante = "abigail.acuna@gmail.com",
                EventoId = 1
            };
            var result = await controller.PostParticipante(nuevoParticipante);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar participante cuando email es nulo
        [Fact]
        public async Task PostParticipante_NoAgregaParticipante_EmailNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = "Abigail Acuña",
                CorreoParticipante = null,
                EventoId = 1
            };
            var result = await controller.PostParticipante(nuevoParticipante);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar participante cuando nombre sea menor al mínimo
        [Fact]
        public async Task PostParticipante_NoAgregaParticipante_NombreMinimo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = "Ab",
                CorreoParticipante = "abigail.acuna@gmail.com",
                EventoId = 1
            };

            var result = await controller.PostParticipante(nuevoParticipante);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del participante debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }
        //Agregar participante cuando nombre sea mayor al máximo
        [Fact]
        public async Task PostParticipante_NoAgregaParticipante_NombreMaximo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = new string('A', 51), 
                CorreoParticipante = "abigail.acuna@gmail.com",
                EventoId = 1
            };

            var result = await controller.PostParticipante(nuevoParticipante);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del participante debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }
        //Agregar participante cuando correo no válido
        [Fact]
        public async Task PostParticipante_NoAgregaParticipante_EmailInvalido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new ParticipantesController(context);
            var nuevoParticipante = new Participante
            {
                NombreParticipante = "Abigail Acuña",
                CorreoParticipante = "email-invalido",  
                EventoId = 1
            };

            var result = await controller.PostParticipante(nuevoParticipante);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El correo electrónico no es válido.", badRequestResult.Value);
        }

    }
}
