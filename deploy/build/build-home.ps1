cd ..\..\Web\Biz.BrightOnion.Web\frontend
npm install
npm run-script build -- --environment=presentation-home
cd ..\..\..
dotnet publish .\Biz.BrightOnion.sln
