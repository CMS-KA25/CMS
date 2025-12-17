using System;

namespace CMS.Application.Auth.DTOs.Requests
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Purpose { get; set; }
    }
}
