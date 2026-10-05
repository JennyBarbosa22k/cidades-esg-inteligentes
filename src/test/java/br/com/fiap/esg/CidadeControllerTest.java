package br.com.fiap.esg;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@SpringBootTest
@AutoConfigureMockMvc
class CidadeControllerTest {

    @Autowired
    private MockMvc mvc;

    @Test
    void deveListarCidades() throws Exception {
        mvc.perform(get("/api/cidades")).andExpect(status().isOk());
    }

    @Test
    void deveCriarCidadeValida() throws Exception {
        String json = """
            {"nome":"Curitiba","estado":"PR","indiceEsg":82,"emissaoCo2":2.1,"areaVerdePercentual":64.5}
            """;
        mvc.perform(post("/api/cidades").contentType(MediaType.APPLICATION_JSON).content(json))
           .andExpect(status().isCreated())
           .andExpect(jsonPath("$.nome").value("Curitiba"));
    }

    @Test
    void deveRejeitarIndiceEsgInvalido() throws Exception {
        String json = """
            {"nome":"Teste","estado":"SP","indiceEsg":150,"emissaoCo2":1.0,"areaVerdePercentual":10}
            """;
        mvc.perform(post("/api/cidades").contentType(MediaType.APPLICATION_JSON).content(json))
           .andExpect(status().isBadRequest());
    }

    @Test
    void deveRetornar404ParaIdInexistente() throws Exception {
        mvc.perform(get("/api/cidades/9999")).andExpect(status().isNotFound());
    }
}
