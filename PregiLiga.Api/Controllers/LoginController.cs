﻿using System.Net;
using System.Web;
using System.Web.Http;
using AcklenAvenue.Data.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using PrediLiga.Data;
using PrediLiga.Domain.Entities;
using PrediLiga.Domain.Services;

namespace PregiLiga.Api.Controllers
{
    public class LoginController:ApiController
    {
        IReadOnlyRepository _readOnlyRepository;

        public LoginController()
        {
            
        }

        public LoginController(IReadOnlyRepository readOnlyRepository )
        {
            
            _readOnlyRepository = readOnlyRepository;
        }

        public AuthModel Post([FromBody]AccountLoginModel model)
        {

            string connectionString = ConnectionStrings.Get();



            MsSqlConfiguration databaseConfiguration =
                MsSqlConfiguration.MsSql2008.ShowSql().ConnectionString(x => x.Is(connectionString));


            ISessionFactory sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build();
            _readOnlyRepository = new ReadOnlyRepository(sessionFactory.OpenSession());

            var user = _readOnlyRepository.FirstOrDefault<Account>(x => x.Email == model.Email);
            if (user != null)
            {
                if (user.CheckPassword(model.Password))
                {
                    var authModel = new AuthModel();
                    authModel.Token = "SuperHash";
                    return authModel;
                }
                throw new HttpException((int)HttpStatusCode.Unauthorized, "Password doesn't match.");
            }
            throw new HttpException((int) HttpStatusCode.NotFound, "User doesn't exist.");
        }

    }

    public class AuthModel
    {
        public string Token { get; set; }
    }

    public class AccountLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}