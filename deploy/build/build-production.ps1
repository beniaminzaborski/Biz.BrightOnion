cd ..\..\Web\Biz.BrightOnion.Web\frontend
npm install
npm run-script build -- --environment=prod
cd ..\..\..
dotnet publish .\Biz.BrightOnion.sln
