//<snippetTicketNumberPlugin>
using System;

// Microsoft Dynamics CRM namespace(s)
using Microsoft.Xrm.Sdk;

namespace TicketNumberPlugin
{
    public class TicketNumberPlugin: IPlugin
    {
        /// <summary>
        /// A plug-in that auto generates an ticket number when an
        /// ticket is created.
		/// </summary>
        /// <remarks>Register this plug-in on the Create message, ticket entity,
        /// and pre-operation stage.
        /// </remarks>
        //<snippetTicketNumberPlugin2>
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                //</snippetticketNumberPlugin2>

                // Verify that the target entity represents an Ticket.
                // If not, this plug-in was not registered correctly.
                if (entity.LogicalName == "new_pricingapprovalticket")
                {
                    // An ticketnumber attribute should not already exist because
                    // it is system generated.
                    if (entity.Attributes.Contains("new_ticketnumber") == false)
                    {
                        // Create a new ticketnumber attribute, set its value, and add
                        // the attribute to the entity's attribute collection.
                        //Random rndgen = new Random();
                        //entity.Attributes.Add("new_ticketnumber", rndgen.Next().ToString());
                        FormattedAlphanumericStrings rndgen = new FormattedAlphanumericStrings();
                        entity.Attributes.Add("new_ticketnumber", rndgen.createTicketNum().ToString());
                    }
                    else
                    {
                        // Throw an error, because ticket numbers must be system generated.
                        // Throwing an InvalidPluginExecutionException will cause the error message
                        // to be displayed in a dialog of the Web application.
                        throw new InvalidPluginExecutionException("The ticket number can only be set by the system.");
                    }

                }
            }
        }
    }
    class FormattedAlphanumericStrings
    {
        // what's available; I have Listed all Letters and numbers
        public static string possibleCharsAndNums = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        // list if numbers
        public static string possibleNums = "0123456789";
        // optimized (?) what's available
        public static char[] possibleCharsAndNumsArray = possibleCharsAndNums.ToCharArray();
        public static char[] possibleNumsArray = possibleNums.ToCharArray();
        // optimized (precalculated) count
        public static int possibleCharsAndNumsAvailable = possibleCharsAndNums.Length;
        public static int possibleNumsAvailable = possibleNums.Length;
        // shared randomization thingy
        public static Random random = new Random();

        public string getRandomChars(int num)
        {
            /*
             use the random method to select a char and repeat 
             * for the numbe of times passed in as a parameter
             */
            var result = new char[num];
            while (num-- > 0)
            {
                result[num] = possibleCharsAndNumsArray[random.Next(possibleCharsAndNumsAvailable)];
            }
            return new string(result);
        }

        public string getRandomNums(int num)
        {
            var result = new char[num];
            while (num-- > 0)
            {
                result[num] = possibleNumsArray[random.Next(possibleNumsAvailable)];
            }
            return new string(result);
        }

        public string createTicketNum()
        {
            // build the desired pattern using combination of chars and numbers
            // in this case I have one fixed value then 4 numbers then 6 charsNnumbers
            var TicketNum = "AYH" + "-" + getRandomNums(4) + "-" + getRandomChars(6);
            
            return TicketNum;

        }
    }
 }
