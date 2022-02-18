#! /bin/bash

while test $# -gt 0; do
        case "$1" in
            -f)
                shift
                mkdir $1
                cd $1
                shift
                ;;
            *)
                echo "$1 is not a recognized flag!"
                return 1;
                ;;
        esac
done 

dotnet new sln
dotnet new webapi -n API
dotnet new classlib -n Services
dotnet new classlib -n Database
dotnet new classlib -n Models
dotnet new classlib -n Providers
dotnet sln add API
dotnet sln add Services
dotnet sln add Database
dotnet sln add Models
dotnet sln add Providers
dotnet sln list
cd API
dotnet add reference ../Services
dotnet add reference ../Providers
cd ..
cd Services
dotnet add reference ../Models
dotnet add reference ../Database
cd ..
cd Providers
dotnet add reference ../Services
cd ..
cd Database
dotnet add reference ../Models 
cd ..
dotnet new gitignore

exit 0