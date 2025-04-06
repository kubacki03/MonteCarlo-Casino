using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;

namespace TestyMonteCarlo
{
    public class ChatBotTests
    {
        [Fact]
        public void GetResponse_ShouldReturnExpectedAnswer_WhenMessageMatchesKey()
        {
          
            var controller = new BotController();
            string inputMessage = "Ile trzeba mieć lat";
            string expectedResponse = "Aby grać w gry w MonteCarlo należy mieć ukończone 18 lat";

          
            var response = controller.getResponse(inputMessage);

          
            Assert.Equal(expectedResponse, response);
        }

     

        [Fact]
        public void GetResponse_ShouldHandlePartialMatches()
        {
           
            var controller = new BotController();
            string inputMessage = "Czy mogę wypłacić pieniądze";
            string expectedResponse = "Tak, aby wypłacić pieniądze na swoje konto bankowe, należy przejść do sekcji 'Saldo', wybrać opcję 'Wypłać' i wprowadzić dane konta bankowego. Pamiętaj, że minimalna kwota wypłaty to 10zł.";

            
            var response = controller.getResponse(inputMessage);

            
            Assert.Equal(expectedResponse, response);
        }
    }
}
