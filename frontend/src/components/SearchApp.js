import React, { useState } from "react";

export default function SearchApp() {
  const [searchTerm, setSearchTerm] = useState("");
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      setError("Please enter a search term.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      const response = await fetch(
        `http://localhost:5228/Search?term=${encodeURIComponent(searchTerm)}`
      );

      const text = await response.text();
      if (!text) {
        throw new Error("No data received from the server.");
      }

      const data = JSON.parse(text);

      if (!response.ok) {
        throw new Error(data.message || "Error fetching search results.");
      }

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

  return (
    <div className="flex flex-col items-center min-h-screen bg-gray-50 p-6">
      {/* App Heading */}
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

      {/* Search Results - Scrollable */}
      <div className="mt-6 w-full max-w-lg max-h-[400px] overflow-y-auto bg-white shadow-md rounded-lg border border-gray-200">
        {results.length > 0
          ? results.map((result) => (
              <div
                key={result.emailId}
                className="p-4 border-b last:border-b-0"
              >
                <strong className="text-lg text-gray-900">
                  {result.emailName}
                </strong>
                <p className="text-sm text-gray-600 mt-1">
                  {result.emailContent}
                </p>
                <p className="text-xs text-gray-500 mt-2">
                  Occurrences: {result.occurrenceCount}
                </p>
              </div>
            ))
          : !loading &&
            !error && <p className="text-gray-500 p-4">No results found.</p>}
      </div>
    </div>
  );
}
