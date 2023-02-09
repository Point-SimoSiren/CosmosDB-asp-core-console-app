using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace CosmosGettingStartedTutorial
{
    class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "NoteDB";
        private string containerId = "Notes";

        // <Main>
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
              

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
       
        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();

            await this.AddItemsToContainerAsync();

            Console.WriteLine("Do you want to show all saved notes now? y (yes) or n (no)");
            string readAnswer = Console.ReadLine();
            if (readAnswer.ToLower() == "y" || readAnswer.ToLower() == "yes")
            await this.QueryItemsAsync();


        }

        private async Task CreateDatabaseAsync()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
  
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }
       

        private async Task AddItemsToContainerAsync()
        {
            Console.WriteLine("Do you want to save a new note? choose y (yes) or n (no)");

            string saveAnswer = Console.ReadLine();

            if (saveAnswer.ToLower() == "y" || saveAnswer.ToLower() == "yes")
            {

                bool imp = false;
                Console.WriteLine("Give note");
                string noteText = Console.ReadLine();

                Console.WriteLine("Is this note of high importance? choose y (yes) or n (no)");

                string impString = Console.ReadLine();

                if (impString.ToLower() == "y" || impString.ToLower() == "yes")
                {
                    imp = true;
                }

                Random rnd1 = new Random();
                string num1 = rnd1.Next().ToString();
                Random rnd2 = new Random();
                string num2 = rnd2.Next().ToString();

                string idString = num1 + num2;

                Notes noteToAdd = new Notes
                {
                    Id = idString,
                    PartitionKey = "NotePartition",
                    Text = noteText,
                    Important = imp,
                    TimeWrote = DateTime.Now
                };

                // Create  a note in the container
                ItemResponse<Notes> noteResponce = await this.container.CreateItemAsync<Notes>(noteToAdd, new PartitionKey(noteToAdd.PartitionKey));

            }
        }
   

       private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c";

            Console.WriteLine("____________Saved notes_____________");

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Notes> queryResultSetIterator = this.container.GetItemQueryIterator<Notes>(queryDefinition);

            List<Notes> notes = new List<Notes>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Notes> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Notes note in currentResultSet)
                {
                    notes.Add(note);
                    if (note.Important == true)
                    {
                        Console.WriteLine(note.Text + " Important!");
                    }
                    else
                    {
                        Console.WriteLine(note.Text);
                    }
                }
            }

            //Dispose of CosmosClient
            this.cosmosClient.Dispose();

        }

        // </QueryItemsAsync>



        // <ReplaceFamilyItemAsync>
        /// <summary>
        /// Replace an item in the container
        /// </summary>
        /*
        private async Task ReplaceFamilyItemAsync()
        {
            ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>("Wakefield.7", new PartitionKey("Wakefield"));
            var itemBody = wakefieldFamilyResponse.Resource;
            
            // update registration status from false to true
            itemBody.IsRegistered = true;
            // update grade of child
            itemBody.Children[0].Grade = 6;

            // replace the item with the updated content
            wakefieldFamilyResponse = await this.container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.PartitionKey));
            Console.WriteLine("Updated Family [{0},{1}].\n \tBody is now: {2}\n", itemBody.LastName, itemBody.Id, wakefieldFamilyResponse.Resource);
        }
        // </ReplaceFamilyItemAsync>

        // <DeleteFamilyItemAsync>
        /// <summary>
        /// Delete an item in the container
        /// </summary>
        private async Task DeleteFamilyItemAsync()
        {
            var partitionKeyValue = "Wakefield";
            var familyId = "Wakefield.7";

            // Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<Family> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Family>(familyId,new PartitionKey(partitionKeyValue));
            Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, familyId);
        }
        // </DeleteFamilyItemAsync>

        // <DeleteDatabaseAndCleanupAsync>
        /// <summary>
        /// Delete the database and dispose of the Cosmos Client instance
        /// </summary>
        private async Task DeleteDatabaseAndCleanupAsync()
        {
            DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", this.databaseId);

            //Dispose of CosmosClient
            this.cosmosClient.Dispose();
        }
        // </DeleteDatabaseAndCleanupAsync>
    */
    }
}
