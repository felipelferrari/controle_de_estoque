# controle_de_estoque

## Categorias

A API disponibiliza CRUD de categorias em `/api/categorias`. Produtos podem receber `idCategoria` opcional em `POST` e `PUT` de `/api/produtos`.

Antes de publicar esta versão, aplique `RecebimentoMercadorias/Data/Sql/20261109_create_categorias.sql` no PostgreSQL. O script cria a tabela e a chave estrangeira; IDs de categoria já presentes em produtos são preservados como categorias temporárias nomeadas `Categoria {id}`.