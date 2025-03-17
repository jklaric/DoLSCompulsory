import React, { useState } from "react";

export default function SearchApp() {
  const [searchTerm, setSearchTerm] = useState("");
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const apiUrl = process.env.REACT_APP_API_URL;

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      setError("Please enter a search term.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      console.log("Fetching from:", apiUrl);
      const response = await fetch(
        `${apiUrl}/Search?term=${encodeURIComponent(searchTerm)}`
      );

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const data = await response.clone().json();

      setResults(data.length ? data : []);
      if (data.length === 0) {
        setError("No results found.");
      }
    } catch (err) {
      setError(err.message);
      setResults([]);
    }

    setLoading(false);
  };

  // Function to trigger file download
  const handleDownload = (fileContent, fileName) => {
    const blob = new Blob([fileContent], { type: "text/plain" });
    const a = document.createElement("a");
    a.href = URL.createObjectURL(blob);
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  };

  return (
    <div className="flex flex-col items-center min-h-screen bg-gray-50 p-6">
      <h1 className="text-3xl font-semibold mb-6 text-gray-800">
        📧 Email Search App
      </h1>

      {/* Search Bar */}
      <div className="flex w-full max-w-lg">
        <input
          type="text"
          placeholder="Enter search term..."
          className="p-3 border border-blue-300 rounded-l w-full focus:outline-none focus:ring-2 focus:ring-blue-400"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <button
          className="bg-blue-600 text-white px-4 py-3 rounded-r hover:bg-blue-700 transition"
          onClick={handleSearch}
        >
          Search
        </button>
      </div>

      {/* Error Message */}
      {error && <p className="text-red-500 mt-3">{error}</p>}

      {/* Loading Spinner */}
      {loading && <p className="mt-3 text-gray-600">Loading...</p>}

      {/* Search Results with Download Buttons */}
      <div className="mt-6 w-full max-w-lg max-h-[400px] overflow-y-auto bg-white shadow-md rounded-lg border border-gray-200">
        {results.length > 0
          ? results.map((file, index) => (
              <div
                key={index}
                className="p-4 border-b last:border-b-0 flex justify-between items-center"
              >
                <div>
                  <strong className="text-lg text-gray-900">
                    {file.emailName}
                  </strong>
                  <p className="text-sm text-gray-600 mt-1">
                    {file.emailContent}
                  </p>
                  <p className="text-xs text-gray-500 mt-2">
                    Occurrences: {file.occurrenceCount}
                  </p>
                </div>
                <button
                  className="bg-green-600 text-white px-3 py-2 rounded hover:bg-green-700 transition"
                  onClick={() =>
                    handleDownload(file.emailContent, `email_${index + 1}.txt`)
                  }
                >
                  ⬇ Download
                </button>
              </div>
            ))
          : !loading &&
            !error && <p className="text-gray-500 p-4">No results found.</p>}
      </div>
    </div>
  );
}
