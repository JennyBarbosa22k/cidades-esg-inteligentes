package br.com.fiap.esg.repository;

import br.com.fiap.esg.model.Cidade;
import org.springframework.data.jpa.repository.JpaRepository;

public interface CidadeRepository extends JpaRepository<Cidade, Long> {
}
