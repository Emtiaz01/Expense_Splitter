# Frontend Setup Instructions

## Quick Start

```bash
cd client
npm install
npm run dev
```

The app will open at `http://localhost:3000`

## Available Scripts

### `npm run dev`

Runs the app in development mode with hot reload.

### `npm run build`

Builds the app for production to the `dist` folder.

### `npm run preview`

Preview the production build locally.

### `npm run lint`

Runs ESLint to check for code issues.

## Project Structure

```
client/
├── public/              # Static assets
├── src/
│   ├── components/      # Reusable components
│   │   ├── Navbar.jsx
│   │   ├── CreateGroupModal.jsx
│   │   ├── AddExpenseModal.jsx
│   │   └── BalanceChart.jsx
│   ├── pages/          # Page components
│   │   ├── Login.jsx
│   │   ├── Register.jsx
│   │   ├── Dashboard.jsx
│   │   ├── GroupPage.jsx
│   │   └── SettlementPage.jsx
│   ├── services/       # API services
│   │   ├── api.js           # Axios configuration
│   │   └── apiService.js    # API methods
│   ├── utils/          # Helper functions
│   │   ├── helpers.js       # General helpers
│   │   └── pdfExport.js     # PDF generation
│   ├── App.jsx         # Main app component
│   ├── main.jsx        # Entry point
│   └── index.css       # Global styles
├── index.html
├── package.json
├── vite.config.js
└── tailwind.config.js
```

## Key Dependencies

- **React 18**: UI library
- **React Router v6**: Client-side routing
- **Axios**: HTTP client for API calls
- **Tailwind CSS**: Utility-first CSS framework
- **Chart.js**: Data visualization
- **jsPDF**: PDF generation

## Environment Variables

Create a `.env` file in the `client` folder:

```env
VITE_API_URL=http://localhost:5000/api
```

## Styling

This project uses Tailwind CSS. Customize colors and styles in `tailwind.config.js`.

### Custom Classes

Defined in `src/index.css`:

- `.btn` - Base button styles
- `.btn-primary` - Primary button
- `.btn-secondary` - Secondary button
- `.btn-danger` - Danger button
- `.input` - Input field styles
- `.card` - Card container styles

## API Integration

All API calls go through `src/services/apiService.js`. The service automatically:

- Adds JWT token to requests
- Handles 401 errors (redirects to login)
- Provides typed methods for all endpoints

Example usage:

```javascript
import { groupService } from "../services/apiService";

const groups = await groupService.getGroups();
```

## State Management

Currently using React's built-in state management (useState, useEffect). For larger apps, consider adding:

- Redux Toolkit
- Zustand
- React Query

## Tips

1. **Hot Reload**: Changes are reflected instantly
2. **Error Boundaries**: Add error boundaries for better error handling
3. **Code Splitting**: Use React.lazy() for route-based code splitting
4. **Performance**: Use React DevTools to profile components

## Building for Production

```bash
npm run build
```

Output will be in `dist/` folder. Deploy this folder to:

- Vercel
- Netlify
- Firebase Hosting
- Any static hosting service

## Troubleshooting

**Issue: Port 3000 in use**

- Vite will automatically use next available port (5173, 5174, etc.)

**Issue: API calls failing**

- Check if backend is running
- Verify VITE_API_URL in .env
- Check browser console for errors

**Issue: Styles not loading**

- Run `npm run build` to rebuild Tailwind
- Clear browser cache
