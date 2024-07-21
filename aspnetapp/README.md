# Matcha_project

start the project with the following command:

start DB:
cd Matcha_project
docker-compose up --build -d


start API:
cd aspnetapp
dotnet build
cd Web_API
dotnet run --launch-profile https

start client:
cd vueapp
npm install
npm run dev


go to https://localhost:5001/swagger/index.html
and seed data

now you can go to https://localhost:8080/login and start using the app