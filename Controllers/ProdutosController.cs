using System;
using System.Linq;
using Api_Rest.Controllers.Models;
using Api_Rest.Data;
using Microsoft.AspNetCore.Mvc;
using Api_Rest.HATEOAS;

namespace Api_Rest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO","GET");
            HATEOAS.AddAction("DELETE_PRODUCT","DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT","PATCH"); 
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var produtos = database.Produtos.ToList();
            return Ok(produtos); //Ok = Status code 200 && dados
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());
                return Ok (produtoHATEOAS);
            }
            //catch (Exception e)
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp)
        {
            //VALIDAÇÃO
            if(pTemp.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Preço inválido"});
            }

            if (pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Nome inválido"});
            }

            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges();

            //return Ok (new {info = "Você criou um novo produto", produto = pTemp});
            //return Ok(new{msg = "Produto craido com sucesso"});
            Response.StatusCode = 201;
            return new ObjectResult("");
        }


        [HttpDelete("{id}")]
        public IActionResult Delete (int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produto);
                database.SaveChanges();
                return Ok ();
            }
            ////catch (Exception e)
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto)
        {
            if(produto.Id > 0)
            {
                try
                {
                    var p = database.Produtos.First(pTemp => pTemp.Id == produto.Id);
                    if(p != null)
                    {
                        //EDITANDO ?=verdadeiro/então :=falso
                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                        p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;
                        database.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "produto não encontrado"});
                    }
                }
                catch
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new {msg ="produto não encontrado"});
                }
                
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id inválido"});
            }
        }




            public class ProdutoTemp
        {
            public string Nome {get; set;}
            public float Preco {get; set;}
        }

        public class ProdutoContainer
        {
            public Produto produto {get; set;}
            public Link[] links {get; set;}
        }
        
    }
}