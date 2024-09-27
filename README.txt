============================================================
          MGLaser - Máquina de Gravação a Laser
============================================================

Desenvolvedor: Pedro Borges
Data de Criação: Setembro/2024
Versão: 1.0.0

============================================================
           Descrição
============================================================
O Software é responsável por realizar as gravações das informações padrões e Seriais (consome o arquivo de OP, .txt, que é baixado do ERP da TECHNOS).

O Software Realiza o carregamento de programas que previamente foram criados dentro do software da Gravadora a Laser, juntamente aos envios das serias e carregamento dos arquivos OP. Realiza o rastreio de quantas peças foram gravadas, marca as seriais já utilizadas, Giro manual da Base (0 e 180), quantia de seriais para serem gravadas, opção que remove marcação de peças já utilizadas, informa o momento que as seriais terminam (nesse momento, não é possível realizar o giro da base), mostra o total de seriais.

============================================================
    Requisitos de Sistema
============================================================
- Sistema Operacional: Windows
- Visual Studio: https://visualstudio.microsoft.com/pt-br/
- Versão do Visual Studio: Microsoft Visual Studio Professional 2022 (64 bits) - Current - Versão 17.9.1
- Estrutura de Destino: .NET Framework 4.8
- Dependências:
  - .NET Framework
  - Using:
    - Using Excel = Microsoft.Office.Interop.Excel
    - Using MBPLib2;
- MBPlusActiveX - Biblioteca da Gravadora a Laser - Os passos de instalação no C# está no Manual de utilização da Keyence: MD-X-ActiveX_UM_H99GB_252167_GB_2073-1

============================================================
     Arquivos do Software
============================================================
1. Clone o repositório ou baixe os arquivos.
2. Uma vez que o arquivo foi baixado, execute: MGLASER.sln
3. O Projeto será aberto no Visual Studio e estará pronto.

O .exe está em /bin/Debug

============================================================
     Autor
============================================================
Desenvolvido por: Pedro Borges
Email de contato: Pedro.borges@deltasollutions.com.b
