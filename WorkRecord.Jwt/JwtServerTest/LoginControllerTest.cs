using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkRecord.JwtServer;
using WorkRecord.Model.Entity;
using WorkRecord.Model.Jwt;
using Xunit;
using Xunit.Abstractions;

namespace WorkRecordTest
{
    public class LoginControllerTest
    {
        /// <summary>
        /// HttpClient对象
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// 用来输出返回值
        /// </summary>
        public ITestOutputHelper Output { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="outputHelper"></param>
        public LoginControllerTest(ITestOutputHelper outputHelper)
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>());
            Client = server.CreateClient();
            Output = outputHelper;
        }

        [Fact]
        public async Task Get_ShouldBe_ok()
        {
            // 2、Act
            var response = await Client.GetAsync("api/login");

            // Output
            string responseText = await response.Content.ReadAsStringAsync();
            // 输出返回信息
            Output.WriteLine(responseText);

            Assert.Equal("Success", responseText);
        }

        /// <summary>
        /// 针对Post进行测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_ShouldBe_ok()
        {
            // 1、Arrange
            User entity = new User()
            {
                Password = "E10ADC3949BA59ABBE56E057F20F883E",
                Account = "admin",
            };

            var str = JsonConvert.SerializeObject(entity);
            HttpContent content = new StringContent(str);



            // 2、Act
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await Client.PostAsync("api/login", content);
            string responseBody = await response.Content.ReadAsStringAsync();
            Output.WriteLine(responseBody);

            ValidateInfo validate = JsonConvert.DeserializeObject<ValidateInfo>(responseBody);
            // 3、Assert
            Assert.Equal(0, validate.Code);
        }
    }
}
