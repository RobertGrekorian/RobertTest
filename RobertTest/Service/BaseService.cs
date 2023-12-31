﻿using RobertTest.Models.Dto;
using RobertTest.Service;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using static RobertTest.Utility.SD;
using System;


namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory,ITokenProvider tokenProvider) 
        { 
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true )
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoApi");
                HttpRequestMessage message = new();
                if(requestDto.contentType == ContentType.MultipartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }
                //token
                if( withBearer )
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
                message.RequestUri = new Uri(requestDto.Url);

                if(requestDto.contentType == ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach(var prop in requestDto.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDto.Data);
                        if(value is FormFile)
                        {
                            var file = (FormFile)value;
                            if(file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()),prop.Name,file.FileName);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
                    }
                    message.Content = content;
                }
                else
                {
                    if (requestDto.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                    }
                }

                
                HttpResponseMessage? apiResponse = null;

                switch(requestDto.ApiType)
                {
                    case ApiType.POST: 
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default: 
                        message.Method = HttpMethod.Get;
                        break;
                }
                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Message = "Not Found"
                        };
                    case HttpStatusCode.BadRequest:
                        var apiContent1 = await apiResponse.Content.ReadAsStringAsync();

                        var m = JsonConvert.DeserializeObject<ResponseDto>(apiContent1);
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Message = m.Message
                };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Message = "Access Denied",
                            Result = new ResponseDto()
                        };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Message = "Unauthorized",
                            Result = new ResponseDto()
                        };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Message = "Internal Server Error",
                            Result = new ResponseDto()
                        };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
                
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }

        }
    }
}
