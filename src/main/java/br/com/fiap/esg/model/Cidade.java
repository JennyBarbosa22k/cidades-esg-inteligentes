package br.com.fiap.esg.model;

import jakarta.persistence.*;
import jakarta.validation.constraints.*;

@Entity
@Table(name = "cidades")
public class Cidade {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    private String nome;

    @NotBlank
    @Size(min = 2, max = 2)
    private String estado;

    /** Indice ESG de 0 a 100 */
    @Min(0) @Max(100)
    private int indiceEsg;

    /** Emissao de CO2 em toneladas por habitante/ano */
    @PositiveOrZero
    private double emissaoCo2;

    /** Percentual de area verde */
    @DecimalMin("0.0") @DecimalMax("100.0")
    private double areaVerdePercentual;

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getNome() { return nome; }
    public void setNome(String nome) { this.nome = nome; }
    public String getEstado() { return estado; }
    public void setEstado(String estado) { this.estado = estado; }
    public int getIndiceEsg() { return indiceEsg; }
    public void setIndiceEsg(int indiceEsg) { this.indiceEsg = indiceEsg; }
    public double getEmissaoCo2() { return emissaoCo2; }
    public void setEmissaoCo2(double emissaoCo2) { this.emissaoCo2 = emissaoCo2; }
    public double getAreaVerdePercentual() { return areaVerdePercentual; }
    public void setAreaVerdePercentual(double v) { this.areaVerdePercentual = v; }
}
