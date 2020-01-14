# Dotace

## aby fungoval importToES

Je zapotřebí, mít tento repozitář na úrovni repozitáře HlidacStatu:

např.

``` cmd

c:\repos\HlidacStatu
c:\repos\Dotace

```


# Pro stažení SZIF dat
Je potřeba mít Ruby (testováno na OS X, Ruby 2.6.3)

Skript si vytvoří datové složky nad složkou repa, kvůli tomu, aby se neindexovaly v IDE.

```shell
cd szif
bundle exec
ruby szif.rb
```
