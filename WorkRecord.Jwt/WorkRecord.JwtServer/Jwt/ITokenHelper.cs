using WorkRecord.Model.Entity;
using WorkRecord.Model.Jwt;

namespace WorkRecord.JwtServer.Jwt
{
    public interface ITokenHelper
    {
        /// <summary>
        /// 根据用户信息返回一个Token
        /// </summary>
        /// <param name="user">登录用户信息</param>
        /// <returns></returns>
        TokenInfo CreateToken(User user);
    }
}
