import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Box, Paper, Typography } from "@mui/material";

export default function SystemResourceAdvice() {
  return (
    <Paper
      sx={{
        maxWidth: "40%",
        marginBottom: 4,
        padding: 2,
        height: "fit-content",
      }}
    >
      <Box
        sx={{ display: "flex", alignItems: "center", gap: 1, marginBottom: 1 }}
      >
        <FontAwesomeIcon
          icon={faCircleInfo}
          style={{ width: 48, height: 48, marginRight: "8px" }}
        />

        <Typography color="textSecondary" align="justify" mb={"4px"}>
          Os recursos representam as permissões do sistema. Um bom ponto de
          partida é pensar neles como endpoints da api.
        </Typography>
      </Box>

      <Typography color="textSecondary" align="justify">
        Apenas Desenvolvedores e Administradores devem ter acesso a esta seção,
        pois a criação ou modificação inadequada de recursos pode comprometer a
        segurança do sistema.
      </Typography>
    </Paper>
  );
}
