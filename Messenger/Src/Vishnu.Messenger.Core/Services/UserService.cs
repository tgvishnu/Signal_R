using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Common.DTO;

namespace Vishnu.Messenger.Core.Services
{
    public class UserService : ServiceBase
    {
        public UserService(string baseAddress)
        {
            base.mBaseAddress = new Uri(baseAddress);
        }

        public async Task<IList<Common.Models.UserDetails>> GetUsers()
        {
            List<Common.Models.UserDetails> resultCollection = new List<Common.Models.UserDetails>();
            resultCollection.Clear();
            using (var client = new HttpClient())
            {
                client.BaseAddress =  base.mBaseAddress;
                var responseTask = await client.GetAsync("users");
                if(responseTask.IsSuccessStatusCode)
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var userCollection = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString);
                        if (userCollection != null)
                        {
                            foreach(var item in userCollection)
                            {
                                resultCollection.Add(new Common.Models.UserDetails()
                                {
                                    EmailId = item.Email,
                                    UserName = item.UserName
                                });
                            }
                        }
                    }
                }
            }

            return resultCollection;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
