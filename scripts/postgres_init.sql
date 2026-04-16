-- Table: categorias
CREATE TABLE IF NOT EXISTS categorias (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome varchar(255) NOT NULL
);

-- Table: produtos
CREATE TABLE IF NOT EXISTS produtos (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome varchar(255) NOT NULL,
    categoria_id integer NOT NULL,
    CONSTRAINT fk_produto_categoria FOREIGN KEY (categoria_id)
        REFERENCES categorias (id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

-- Table: utilizadores
CREATE TABLE IF NOT EXISTS utilizadores (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome varchar(255) NOT NULL,
    email varchar(320) NOT NULL UNIQUE,
    password_hash text NOT NULL,
    password_salt text NOT NULL
);

-- Table: movimentos
CREATE TABLE IF NOT EXISTS movimentos (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome varchar(255) NOT NULL,
    timestamp timestamptz NOT NULL,
    produto_id integer NOT NULL,
    delta integer NOT NULL,
    descricao text,
    utilizador_id integer NOT NULL,
    CONSTRAINT fk_movimento_produto FOREIGN KEY (produto_id)
        REFERENCES produtos (id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_movimento_utilizador FOREIGN KEY (utilizador_id)
        REFERENCES utilizadores (id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);