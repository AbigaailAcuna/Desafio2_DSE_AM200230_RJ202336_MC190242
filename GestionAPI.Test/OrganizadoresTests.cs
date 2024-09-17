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
    public class OrganizadoresTests
    {
        //agregar organizador cuando todo es válido
        [Fact]
        public async Task PostOrganizador_AgregaOrganizador_OrganizadorValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = "Abigail Acuña",
                CargoOrganizador = "Administradora",
                EventoId = 1
            };

            var result = await controller.PostOrganizador(nuevoOrganizador);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var organizador = Assert.IsType<Organizador>(createdResult.Value);
            Assert.Equal("Abigail Acuña", organizador.NombreOrganizador);
        }
        //ver organizador cuando id es válido
        [Fact]
        public async Task GetOrganizador_RetornaOrganizador_CuandoIdValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = "Abigail Acuña",
                CargoOrganizador = "Administradora",
                EventoId = 1
            };
            context.Organizadores.Add(nuevoOrganizador);
            await context.SaveChangesAsync();
            var result = await controller.GetOrganizador(nuevoOrganizador.Id);
            var actionResult = Assert.IsType<ActionResult<Organizador>>(result);
            var returnValue = Assert.IsType<Organizador>(actionResult.Value);
            Assert.Equal("Abigail Acuña", returnValue.NombreOrganizador);
        }
        [Fact]
        //ver Organizador cuando id no es válido
        public async Task GetOrganizador_RetornaOrganizador_CuandoIdNoValido()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);


            var result = await controller.GetOrganizador(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }
        //Agregar Organizador cuando nombre es nulo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_NombreNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = null,
                CargoOrganizador = "Administradora",
                EventoId = 1
            };
            var result = await controller.PostOrganizador(nuevoOrganizador);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar Organizador cuando cargo es nulo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_CargoNulo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = "Abigail Acuña",
                CargoOrganizador = null,
                EventoId = 1
            };
            var result = await controller.PostOrganizador(nuevoOrganizador);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        //Agregar Organizador cuando nombre sea menor al mínimo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_NombreMinimo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = "Ab",
                CargoOrganizador = "Administradora",
                EventoId = 1
            };

            var result = await controller.PostOrganizador(nuevoOrganizador);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del organizador debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }
        //Agregar Organizador cuando nombre sea mayor al máximo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_NombreMaximo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = new string('A', 51),
                CargoOrganizador = "Administradora",
                EventoId = 1
            };

            var result = await controller.PostOrganizador(nuevoOrganizador);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El nombre del organizador debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }
        //Agregar Organizador cuando cargo sea menor al mínimo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_CargoMinimo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador = "Abigail Acuña",
                CargoOrganizador = "Ad",
                EventoId = 1
            };

            var result = await controller.PostOrganizador(nuevoOrganizador);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El cargo del organizador debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }
        //Agregar Organizador cuando nombre sea mayor al máximo
        [Fact]
        public async Task PostOrganizador_NoAgregaOrganizador_CargoMaximo()
        {
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new OrganizadoresController(context);
            var nuevoOrganizador = new Organizador
            {
                NombreOrganizador =  "Abigail Acuña",
                CargoOrganizador = new string('A', 51),
                EventoId = 1
            };

            var result = await controller.PostOrganizador(nuevoOrganizador);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El cargo del organizador debe tener entre 3 y 50 caracteres.", badRequestResult.Value);
        }


    }
}
