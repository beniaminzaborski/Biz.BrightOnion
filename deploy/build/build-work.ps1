cd ..\..\Web\Biz.BrightOnion.Web\frontend
npm install
npm run-script build -- --environment=presentation-work
cd ..\..\..
dotnet publish .\Biz.BrightOnion.sln
