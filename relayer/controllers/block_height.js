
export async function getBlockHeight(blockID) {
  let block_height = null;

try {
     // Import fetch if running in Node.js environment
     const fetch = (await import('node-fetch')).default;
     
     const url = `https://rest-testnet.onflow.org/v1/blocks/${blockID}`;
     console.log('Fetching block data from:', url);
     
     const response = await fetch(url, {
       method: 'GET',
       headers: {
         'Accept': 'application/json',
         'Content-Type': 'application/json'
       }
     });

     if (!response.ok) {
       console.error('Block fetch failed:', response.status, response.statusText);
       const errorText = await response.text();
       console.error('Error response:', errorText);
       return `({
         error: 'Failed to fetch block data',
         status: response.status,
         message: errorText
       })`;
     }

     const data = await response.json();
     console.log(data)

     block_height = data[0].header.height;
     console.log('Block data height successfully:', block_height);
   
     
     return (block_height);

   } catch (error) {
     console.error('Error fetching block data:', error.message);
     console.error('Full error:', error);
     return `({
       error: 'Internal Server Error',
       message: error.message,
       blockId: blockID
     })`;
   }
}