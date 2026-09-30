import { useState } from "react";
import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";

function App() {
  const [selectedSong, setSelectedSong] = useState(null);

  if (selectedSong) {
    return (
      <SongDetails
        song={selectedSong}
        onBack={() => setSelectedSong(null)}
      />
    );
  }

  return <Catalog onSelectSong={setSelectedSong} />;
}

export default App;