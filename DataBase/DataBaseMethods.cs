using Microsoft.EntityFrameworkCore;
using WPF_HTTP_SERVER.DataBase.Models;

namespace WPF_HTTP_SERVER.DataBase
{
    public static class DataBaseMethods
    {
        public async static Task<bool> CheckUser(UserModel user)
        {
            bool exist = false;

            using (var dataBaseConfiguration = new DataBaseConfiguration())
            {
                var dbUser = await dataBaseConfiguration.Users.SingleOrDefaultAsync(
                    x => x.Login == user.Login &&
                    x.Password == user.Password);

                if (dbUser is not null)
                    exist = true;
            }

            return exist;
        }

        public async static Task<PredictionModel[]> Getpredictions()
        {
            PredictionModel[] predictions;

            using (var dataBaseConfiguration = new DataBaseConfiguration())
            {
                if(dataBaseConfiguration.Predictions.Count() == 0)
                    return new PredictionModel[0];

                predictions = await dataBaseConfiguration.Predictions.ToArrayAsync();
            }

            return predictions ?? new PredictionModel[0];
        }
    }
}
