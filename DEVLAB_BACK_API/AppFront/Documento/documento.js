const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = input.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do client e selecione um arquivo");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST";
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!");
        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else {
        const erro = await response.json();
        alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
    }
    async function buscarDocumentos() {

        const codigoCliente = document.getElementById("codigoCliente").value;

        if (!codigoCliente) {
            alert("Informe o código do cliente.");
            return;
        }

        try {

            const response = await fetch(`${URL_API}/cliente/${codigoCliente}`);

            if (!response.ok) {
                throw new Error("Não foi possível buscar os documentos.");
            }

            const documentos = await response.json();

            const tabela = document.getElementById("tabelaDocumentos");

            tabela.innerHTML = "";

            documentos.forEach(documento => {

                const linha = document.createElement("tr");

                linha.innerHTML = `
            <td>${documento.id}</td>
            <td>${documento.nome}</td>
            <td>${documento.extensao}</td>
            <td>
                <button class="btn-baixar"
                    onclick="baixarDocumento(${documento.id})">
                    Baixar
                </button>

                <button class="btn-excluir"
                    onclick="excluirDocumento(${documento.id})">
                    Excluir
                </button>
            </td>
        `;

                tabela.appendChild(linha);

            });

        } catch (erro) {

            console.error(erro);
            alert("Erro ao buscar os documentos.");

        }

    }