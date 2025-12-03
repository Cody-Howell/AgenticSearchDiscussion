import { marked } from 'marked';
import DOMPurify from 'dompurify';


export const MarkdownDisplay: React.FC<{ value: string, className: string }> = ({ value, className }) => {
  const markedReturn: string = marked.parse(value) as string;

  const clean = DOMPurify.sanitize(markedReturn);

  return <div
    className={"ai-return " + className}
    dangerouslySetInnerHTML={{ __html: clean }} />
}