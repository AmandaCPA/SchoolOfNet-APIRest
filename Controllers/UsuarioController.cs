using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Api_Rest.Data;
using Api_Rest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api_Rest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuarioController(ApplicationDbContext database)
        {
            this.database = database;
        }


        [HttpPost("registro")]
        public IActionResult Registro ([FromBody] Usuario usuario)
        {
            //CRIAÇÃO DE REGISTRO FALHA, POIS NÃO TEM:
            //Verificação se as credenciais são validas
            //Verificação se o e-mail já está cadastrado no banco
            //Encriptar senhas
            database.Add(usuario);
            database.SaveChanges();
            return Ok(new{msg = "Usuário cadastrado com sucesso"});
        }

        [HttpPost("Login")]
        public IActionResult Login ([FromBody] Usuario credenciais)
        {
            //buscar um usuario por email
            //verificar se a senha esta correta
            //gerar um token JWT e retornar esse token para o usuario
            
            try
            {
                Usuario usuario = database.Usuarios.First(User => User.Email.Equals(credenciais.Email));
                if(usuario != null)
                {
                    //achou um usuario com cadastro válido
                    if(usuario.Senha.Equals(credenciais.Senha))
                    {
                        //usuario acertou a senha => logou
                        //gerar o token JWT

                        String chaveDeSeguranca = "school_of_net_manda_muito_bem!";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);
                        
                        var JWT = new JwtSecurityToken(
                            issuer: "sonmarket.com", //Quem esta fornecendo o JWT para o usuario
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso
                        );
                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    }                      
                    else
                    {
                        //não existe nenhum usuario com este e-mail
                        Response.StatusCode = 401; //Não autorizado
                        return new ObjectResult("");
                    }
                }        
                else
                {
                    //não existe nenhum usuario com este e-mail
                    Response.StatusCode = 401; //Não autorizado
                    return new ObjectResult("");
                }    
            }                    
            catch(Exception)
            {
                //não existe nenhum usuario com este e-mail
                Response.StatusCode = 401;
                return new ObjectResult("");
            }    
        }       
    }
}
