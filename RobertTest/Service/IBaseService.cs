using RobertTest.Models.Dto;

namespace RobertTest.Service
{
    public interface IBaseService
    {
        public Task<ResponseDto?> SendAsync(RequestDto requestDto,bool withBearer = true);
    }
}
