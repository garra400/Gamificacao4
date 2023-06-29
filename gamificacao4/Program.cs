using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using gamificacao4.Models;
using gamificacao4.UI;
using gamificacao4.DbContext;

public class Program
{
    private static readonly string connectionString = "server=localhost;user=root;password=;";// Deixar a senha vazia
    private static readonly string databaseName = "db_gamificacao4";
    private static readonly Repositorio<Produto> repositorioProdutos = new Repositorio<Produto>(connectionString);

    private static long proximoIdProduto = 1;

    static void Main(string[] args)
    {
        string connectionStringWithDatabase = $"{connectionString};database={databaseName}";

        DbContext.CriarBancoDeDados(connectionStringWithDatabase);

        int opcao;

        do
        {
            MostrarMenu();
            Console.Write("Digite a opção desejada: ");
            opcao = LerOpcao();

            switch (opcao)
            {
                case 1:
                    ListarProdutos();
                    break;
                case 2:
                    ComprarProduto();
                    break;
                case 3:
                    AdicionarProduto();
                    break;
                case 4:
                    AtualizarProduto();
                    break;
                case 5:
                    RemoverProduto();
                    break;
                case 0:
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine();
        } while (opcao != 0);
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("=========== MENU ===========");
        Console.WriteLine("1. Listar todos os produtos");
        Console.WriteLine("2. Comprar um produto");
        Console.WriteLine("3. Adicionar um novo produto");
        Console.WriteLine("4. Atualizar informações de um produto");
        Console.WriteLine("5. Remover um produto");
        Console.WriteLine("0. Sair");
        Console.WriteLine("============================");
    }

    private static int LerOpcao()
    {
        int opcao;
        while (!int.TryParse(Console.ReadLine(), out opcao))
        {
            Console.WriteLine("Opção inválida. Digite novamente: ");
        }
        return opcao;
    }

    private static void ListarProdutos()
    {
        Console.WriteLine("=== Lista de Produtos ===");
        var produtos = repositorioProdutos.ListarTodos();
        foreach (var produto in produtos)
        {
            Console.WriteLine($"ID: {produto.ProdutoID}, Nome: {produto.Nome}, Preço: {produto.Preco}, Quantidade: {produto.Quantidade}");
        }
    }

    private static void ComprarProduto()
    {
        Console.WriteLine("Digite o ID do produto que deseja comprar:");
        long produtoId = LerLong();

        Console.WriteLine("Digite a quantidade desejada:");
        int quantidadeCompra = LerInt();

        Produto produtoSelecionado = repositorioProdutos.ListarTodos().Find(p => p.ProdutoID == produtoId);

        if (produtoSelecionado != null && produtoSelecionado.Quantidade >= quantidadeCompra)
        {
            produtoSelecionado.Quantidade -= quantidadeCompra;
            repositorioProdutos.Atualizar(produtoSelecionado, "ProdutoID", produtoSelecionado.ProdutoID);
            Console.WriteLine("Compra realizada com sucesso!");
        }
        else
        {
            Console.WriteLine("Não há estoque suficiente para realizar a compra.");
        }
    }

    private static void AdicionarProduto()
    {
        Console.WriteLine("Digite o nome do produto:");
        string nome = Console.ReadLine();

        Console.WriteLine("Digite a descrição do produto:");
        string descricao = Console.ReadLine();

        Console.WriteLine("Digite o preço do produto:");
        decimal preco = LerDecimal();

        Console.WriteLine("Digite a quantidade do produto:");
        int quantidade = LerInt();

        double novoPreco = (double)preco;

        Produto novoProduto = new Produto(proximoIdProduto++, nome, descricao, novoPreco, quantidade);
        repositorioProdutos.Inserir(novoProduto);

        Console.WriteLine("Novo produto adicionado com sucesso!");
    }

    private static void AtualizarProduto()
    {
        Console.WriteLine("Digite o ID do produto que deseja atualizar:");
        long produtoId = LerLong();

        Produto produtoSelecionado = repositorioProdutos.ListarTodos().Find(p => p.ProdutoID == produtoId);

        if (produtoSelecionado != null)
        {
            Console.WriteLine($"Produto encontrado: {produtoSelecionado.Nome}");
            Console.WriteLine("Digite o novo nome do produto (ou deixe em branco para manter o mesmo):");
            string? novoNome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novoNome))
            {
                produtoSelecionado.Nome = novoNome;
            }

            Console.WriteLine("Digite a nova descrição do produto (ou deixe em branco para manter a mesma):");
            string? novaDescricao = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novaDescricao))
            {
                produtoSelecionado.Descricao = novaDescricao;
            }

            Console.WriteLine("Digite o novo preço do produto (ou deixe em branco para manter o mesmo):");
            string novoPrecoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novoPrecoStr) && decimal.TryParse(novoPrecoStr, out decimal novoPreco))
            {
                double novoPrecoDouble = (double)novoPreco;
                produtoSelecionado.Preco = novoPrecoDouble;
            }

            Console.WriteLine("Digite a nova quantidade do produto (ou deixe em branco para manter a mesma):");
            string novaQuantidadeStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novaQuantidadeStr) && int.TryParse(novaQuantidadeStr, out int novaQuantidade))
            {
                produtoSelecionado.Quantidade = novaQuantidade;
            }

            repositorioProdutos.Atualizar(produtoSelecionado, "ProdutoID", produtoSelecionado.ProdutoID);
            Console.WriteLine("Produto atualizado com sucesso!");
        }
        else
        {
            Console.WriteLine("Produto não encontrado.");
        }
    }

    private static void RemoverProduto()
    {
        Console.WriteLine("Digite o ID do produto que deseja remover:");
        long produtoId = LerLong();

        Produto produtoSelecionado = repositorioProdutos.ListarTodos().Find(p => p.ProdutoID == produtoId);

        if (produtoSelecionado != null)
        {
            repositorioProdutos.Remover("ProdutoID", produtoSelecionado.ProdutoID);
            Console.WriteLine("Produto removido com sucesso!");
        }
        else
        {
            Console.WriteLine("Produto não encontrado.");
        }
    }

    private static long LerLong()
    {
        long valor;
        while (!long.TryParse(Console.ReadLine(), out valor))
        {
            Console.WriteLine("Valor inválido. Digite novamente: ");
        }
        return valor;
    }

    private static int LerInt()
    {
        int valor;
        while (!int.TryParse(Console.ReadLine(), out valor))
        {
            Console.WriteLine("Valor inválido. Digite novamente: ");
        }
        return valor;
    }

    private static decimal LerDecimal()
    {
        decimal valor;
        while (!decimal.TryParse(Console.ReadLine(), out valor))
        {
            Console.WriteLine("Valor inválido. Digite novamente: ");
        }
        return valor;
    }
}
