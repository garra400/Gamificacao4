using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using gamificacao4.Models;

namespace gamificacao4.Models;
public class Produto
{
    public long ProdutoID;
    private string? _nome;
    private string? _descricao;
    private double _preco;
    private double _quantidade;

    public Produto(long produtoID, string nome, string descricao, double preco, int quantidade)
    {
        ProdutoID = produtoID;
        _nome = nome;
        _descricao = descricao;
        _preco = preco;
        _quantidade = quantidade;
    }

    public string Nome
    {
        get { return _nome; }
        set { _nome = value; }
    }
    public string Descricao
    {
        get { return _descricao; }
        set { _descricao = value; }
    }
    public double Preco
    {
        get { return _preco; }
        set { _preco = value; }
    }
    public double Quantidade
    {
        get { return _quantidade; }
        set { _quantidade = value; }
    }
}