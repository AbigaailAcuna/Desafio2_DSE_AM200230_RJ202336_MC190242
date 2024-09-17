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
    public class EventosTests
    {
        //agregar evento cuando todo es válido
        [Fact]
        public async Task PostEvento_AgregaEvento_EventoValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2024,10,16),
                LugarEvento = "Iglesia"
            };

            var result = await controller.PostEvento(nuevoEvento);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var evento = Assert.IsType<Evento>(createdResult.Value);
            Assert.Equal("Matrimonio", evento.NombreEvento);
        }
        //ver evento cuando id es válido
        [Fact]
        public async Task GetEvento_RetornaEvento_CuandoIdValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = "Iglesia"
            };
            context.Eventos.Add(nuevoEvento);
            await context.SaveChangesAsync();
            var result = await controller.GetEvento(nuevoEvento.Id);
            var actionResult = Assert.IsType<ActionResult<Evento>>(result);
            var returnValue = Assert.IsType<Evento>(actionResult.Value);
            Assert.Equal("Matrimonio", returnValue.NombreEvento);
        }
        [Fact]
        //ver evento cuando id no es válido
        public async Task GetEvento_RetornaEvento_CuandoIdNoValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);


            var result = await controller.GetEvento(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }
        //Agregar evento cuando nombre es nulo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_NombreNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = null,
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = "Iglesia"
            };
            var result = await controller.PostEvento(nuevoEvento);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar evento cuando FechaEvento es nulo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_FechaNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = default,
                LugarEvento = "Iglesia"
            };
            var result = await controller.PostEvento(nuevoEvento);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar evento cuando LugarEvento es nulo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_LugarNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = null
            };
            var result = await controller.PostEvento(nuevoEvento);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar evento cuando nombre sea menor al mínimo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_NombreMinimo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Mat",
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = "Iglesia"
            };

            var result = await controller.PostEvento(nuevoEvento);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del evento debe tener entre 5 y 100 caracteres.", badRequestResult.Value);
        }
        //Agregar evento cuando nombre sea mayor al máximo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_NombreMaximo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = new string('A', 101),
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = "Iglesia"
            };

            var result = await controller.PostEvento(nuevoEvento);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del evento debe tener entre 5 y 100 caracteres.", badRequestResult.Value);
        }
        //Agregar evento cuando lugar sea menor al mínimo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_LugarMinimo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = "Igl"
            };

            var result = await controller.PostEvento(nuevoEvento);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El lugar del evento debe tener entre 5 y 100 caracteres.", badRequestResult.Value);
        }
        //Agregar evento cuando lugar sea mayor al máximo
        [Fact]
        public async Task PostEvento_NoAgregaEvento_LugarMaximo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2024, 10, 16),
                LugarEvento = new string('A', 101)
            };

            var result = await controller.PostEvento(nuevoEvento);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El lugar del evento debe tener entre 5 y 100 caracteres.", badRequestResult.Value);
        }
        //Agregar evento cuando fecha no válida
        [Fact]
        public async Task PostEvento_NoAgregaEvento_FechaInvalida()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new EventosController(context);
            var nuevoEvento = new Evento
            {
                NombreEvento = "Matrimonio",
                FechaEvento = new DateTime(2020, 10, 16),
                LugarEvento = "Iglesia"
            };

            var result = await controller.PostEvento(nuevoEvento);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("La fecha del evento no puede ser nula y debe ser válida", badRequestResult.Value);
        }

    }
}
