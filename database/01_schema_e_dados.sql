-- ==========================================
-- ETAPA 3: CRIAÇÃO DAS TABELAS (Seções 4.1 - 4.3)
-- ==========================================

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_condicao_jogo),
    UNIQUE KEY uk_condicao_jogo_ds (ds_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_perfil),
    UNIQUE KEY uk_perfil_ds (ds_perfil)
);

CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(100) NOT NULL,
    no_fabricante VARCHAR(100) NOT NULL,
    nu_ano_lancamento INT NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE KEY uk_plataforma_nome (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_seq_genero),
    UNIQUE KEY uk_genero_nome (no_genero)
);

CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento INT NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_jogo),
    INDEX idx_jogo_nome (no_jogo),
    CONSTRAINT fk_jogo_plataforma FOREIGN KEY (co_plataforma) REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo FOREIGN KEY (co_jogo) REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero FOREIGN KEY (co_genero) REFERENCES tb_genero (co_seq_genero)
);

CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(150) NOT NULL,
    ds_email VARCHAR(150) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil INT NOT NULL,
    PRIMARY KEY (co_seq_usuario),
    UNIQUE KEY uk_usuario_email (ds_email),
    CONSTRAINT fk_usuario_perfil FOREIGN KEY (co_perfil) REFERENCES tb_perfil (co_perfil)
);

-- ==========================================
-- ETAPA 4: DADOS DE EXEMPLO
-- ==========================================

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
(1, 'Novo'),
(2, 'Usado'),
(3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
(1, 'Administrador'),
(2, 'Cliente');

INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
(1, 'Super Nintendo', 'Nintendo', 1990),
(2, 'Mega Drive', 'Sega', 1988),
(3, 'PlayStation', 'Sony', 1994),
(4, 'Nintendo 64', 'Nintendo', 1996),
(5, 'Game Boy Advance', 'Nintendo', 2001),
(6, 'Atari 2600', 'Atari', 1977);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
(1, 'Plataforma'),
(2, 'RPG'),
(3, 'Ação'),
(4, 'Aventura'),
(5, 'Luta'),
(6, 'Corrida'),
(7, 'Puzzle'),
(8, 'Esporte');

INSERT INTO tb_jogo (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES
(1, 'Super Mario World', 'Cartucho original americano com label preservado.', 1990, 180.00, 5, TRUE, UTC_TIMESTAMP(), 1, 2),
(2, 'The Legend of Zelda: A Link to the Past', 'Clássico de aventura com bateria interna substituída.', 1991, 250.00, 3, TRUE, UTC_TIMESTAMP(), 1, 3),
(3, 'Chrono Trigger', 'Edição de colecionador em excelente estado.', 1995, 490.00, 1, TRUE, UTC_TIMESTAMP(), 1, 2),
(4, 'Donkey Kong Country', 'Cartucho revisado e testado.', 1994, 160.00, 4, TRUE, UTC_TIMESTAMP(), 1, 3),
(5, 'Sonic the Hedgehog 2', 'Acompanha caixa plástica original.', 1992, 130.00, 6, TRUE, UTC_TIMESTAMP(), 2, 2),
(6, 'Streets of Rage 2', 'Clássico beat em up do Mega Drive.', 1992, 195.00, 2, TRUE, UTC_TIMESTAMP(), 2, 2),
(7, 'Phantasy Star IV', 'RPG icônico da Sega.', 1993, 320.00, 1, TRUE, UTC_TIMESTAMP(), 2, 2),
(8, 'Castlevania: Symphony of the Night', 'Disco sem riscos, acompanha manual.', 1997, 420.00, 2, TRUE, UTC_TIMESTAMP(), 3, 2),
(9, 'Final Fantasy VII', 'Edição Black Label com 3 discos.', 1997, 350.00, 3, TRUE, UTC_TIMESTAMP(), 3, 2),
(10, 'Metal Gear Solid', 'Caixa dupla completa em ótimo estado.', 1998, 280.00, 2, TRUE, UTC_TIMESTAMP(), 3, 2),
(11, 'Gran Turismo 2', 'Dois discos: Arcade e Simulation.', 1999, 110.00, 4, TRUE, UTC_TIMESTAMP(), 3, 3),
(12, 'Super Mario 64', 'Cartucho original testado.', 1996, 210.00, 5, TRUE, UTC_TIMESTAMP(), 4, 2),
(13, 'The Legend of Zelda: Ocarina of Time', 'Clássico absoluto do Nintendo 64.', 1998, 290.00, 2, TRUE, UTC_TIMESTAMP(), 4, 2),
(14, 'Mario Kart 64', 'Ideal para multiplayer local.', 1996, 230.00, 0, TRUE, UTC_TIMESTAMP(), 4, 2),
(15, 'GoldenEye 007', 'Cartucho em ótimo estado de conservação.', 1997, 190.00, 3, TRUE, UTC_TIMESTAMP(), 4, 2),
(16, 'Pokémon Emerald', 'Bateria de relógio interno trocada.', 2004, 340.00, 2, TRUE, UTC_TIMESTAMP(), 5, 3),
(17, 'Metroid Fusion', 'Cartucho original lacrado.', 2002, 450.00, 1, TRUE, UTC_TIMESTAMP(), 5, 1),
(18, 'Tetris', 'Cartucho clássico de puzzle.', 1989, 95.00, 7, TRUE, UTC_TIMESTAMP(), 5, 2),
(19, 'Enduro', 'Cartucho original Atari.', 1983, 75.00, 4, TRUE, UTC_TIMESTAMP(), 6, 2),
(20, 'River Raid', 'Item de catálogo desativado para testes.', 1982, 85.00, 1, FALSE, UTC_TIMESTAMP(), 6, 2);

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
(1, 1), (1, 4),
(2, 2), (2, 3), (2, 4),
(3, 2), (3, 4),
(4, 1), (4, 4),
(5, 1), (5, 3),
(6, 3), (6, 5),
(7, 2),
(8, 3), (8, 4), (8, 2),
(9, 2), (9, 4),
(10, 3), (10, 4),
(11, 6), (11, 8),
(12, 1), (12, 4),
(13, 2), (13, 3), (13, 4),
(14, 6),
(15, 3),
(16, 2), (16, 4),
(17, 3), (17, 4),
(18, 7),
(19, 6),
(20, 3);
