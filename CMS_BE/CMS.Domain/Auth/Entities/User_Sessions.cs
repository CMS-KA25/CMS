using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Auth.Entities
{
    public class User_Sessions
    {
        public Guid SessionID { get; set; }
        public Guid UserID { get; set; }
        public string SessionToken { get; set; }
        public string DeviceInfo { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LoginTimestamp { get; set; }
        public DateTime LastActivityTimestamp { get; set; }
        public DateTime ExpiryTimestamp { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LogoutTimestamp { get; set; }
        public string LogoutReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation property
        public User? User { get; set; }
    }
}
