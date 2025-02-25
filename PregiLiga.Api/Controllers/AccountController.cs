﻿using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Http;
using AcklenAvenue.Data.NHibernate;
using AttributeRouting.Web.Mvc;
using AutoMapper;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Mapping;
using PrediLiga.Data;
using PrediLiga.Domain.Entities;
using PrediLiga.Domain.Services;
using PregiLiga.Api.Models;

namespace PregiLiga.Api.Controllers
{
    public class AccountController:BaseApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;


        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository, IMappingEngine mappingEngine )
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [HttpPost]
        [AcceptVerbs("POST","HEAD")]
        [POST("register")]
        public Account Register([FromBody] AccountRegisterModel model)
        {
            var newUser = _mappingEngine.Map<AccountRegisterModel, Account>(model);
            var createdUser = _writeOnlyRepository.Create(newUser);
            var x = Request;            
            return createdUser;
            
        }


        [HttpPost]
        [AcceptVerbs("POST","HEAD")]
        [POST("login")]
        public AuthModel Login([FromBody] AccountLoginModel model)
        {
            var user = _readOnlyRepository.FirstOrDefault<Account>(x => x.Email == model.Email);
            if (user == null) throw new HttpException((int) HttpStatusCode.NotFound, "User doesn't exist.");
            if (!user.CheckPassword(model.Password))
                throw new HttpException((int) HttpStatusCode.Unauthorized, "Password doesn't match.");
            
            var authModel = new AuthModel
            {
                email = user.Email,
                access_token = AuthRequestFactory.BuildEncryptedRequest(user.Email),
                role = new RoleModel
                {
                    bitMask = 2, title = "admin"
                }
            };

            return authModel;
        }


    }

    public class LeaguesController:BaseApiController
    {
        public LeaguesController()
        {
            
        }

        public List<LeaguesModel> GetLeagues()
        {
            var userTokenModel = GetUserTokenModel();
            if (userTokenModel == null)
                throw new HttpException((int)HttpStatusCode.Unauthorized, "User is not authorized");

            return new List<LeaguesModel>();
        }

    }
}